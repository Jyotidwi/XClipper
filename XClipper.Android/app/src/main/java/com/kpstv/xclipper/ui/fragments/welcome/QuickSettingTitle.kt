package com.kpstv.xclipper.ui.fragments.welcome

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import androidx.fragment.app.Fragment
import androidx.navigation.fragment.findNavController
import com.kpstv.xclipper.R
import com.kpstv.xclipper.extensions.utils.WelcomeUtils.Companion.setUpFragment
import kotlinx.android.synthetic.main.item_gifview.view.*

class QuickSettingTitle : Fragment(R.layout.fragment_welcome) {
    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        setUpFragment(
            view = view,
            activity = requireActivity(),
            paletteId = R.color.palette5,
            nextPaletteId = R.color.palette6,
            textId = R.string.palette5_text,
            nextTextId = R.string.next_6,
            action = {
                findNavController().navigate(QuickSettingTitleDirections.actionQuickSettingTitleToWindowApp())
            },
            insertView = LayoutInflater.from(requireContext()).inflate(
                R.layout.item_gifview, null
            ).apply {
                gifImageView.setImageResource(R.drawable.feature_quicksetting)
            }
        )
    }
}