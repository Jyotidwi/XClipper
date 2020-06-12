package com.kpstv.xclipper.data.localized

import androidx.lifecycle.LiveData
import androidx.paging.DataSource
import androidx.room.*
import com.kpstv.xclipper.data.model.Clip
import java.util.*

@Dao
interface ClipDataDao {
    @Insert(onConflict = OnConflictStrategy.REPLACE)
    fun insert(clip: Clip)

    @Update
    fun update(clip: Clip)

    @Delete
    fun delete(clip: Clip)

    @Query("update table_clip set data = :encryptedData, time = :time where id = :id")
    fun update(id: Int, encryptedData: String, time: Date)

    @Query("update table_clip set isPinned = :isPinned")
    fun update(isPinned: Boolean)

    @Query("delete from table_clip where id = :id")
    fun delete(id: Int)

    @Query("delete from table_clip where id = (select MIN(id) from table_clip);")
    fun deleteLast()

    @Query("select * from table_clip where id = :id")
    fun getData(id: Int): Clip

    @Query("select * from table_clip")
    fun getAllData(): List<Clip>

    @Query("select * from table_clip order by isPinned")
    fun getAllLiveData(): LiveData<List<Clip>>

    @Query("select * from table_clip order by isPinned")
    fun getDataSource(): DataSource.Factory<Int, Clip>
}